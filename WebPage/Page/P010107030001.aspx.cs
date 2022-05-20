//******************************************************************
//*  作    者：
//*  功能說明：重覆件報表查詢
//*  創建日期：2014/08/18
//*  修改記錄：2021/01/13_Ares_Stanley-新增NPOI; 2021/04/01_Ares_Stanley-移除MicrosoftExcel
//*  <author>            <time>            <TaskID>                <desc>
//*  Ares Luke          2020/11/19         20200031-CSIP EOS       調整取web.config加解密參數
//*******************************************************************
//20160601 (U) by Tank, add 最後重覆日
//20160907 (U) by Tank, 調整重覆件邏輯
//20161021 (U) by Tank, timeout=240 設時間為4分鐘
//20170412 (U) by Tank, 設定ScriptManager's AsyncPostBackTimeout

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
using CSIPKeyInGUI.BusinessRules;
using CSIPKeyInGUI.EntityLayer;
using Framework.Common.Message;
using Framework.Data.OM;
using Framework.Data.OM.Collections;
using Framework.WebControls;
using Framework.Common.Logging;
using Framework.Common.Utility;
using CSIPCommonModel.EntityLayer;
using CSIPCommonModel.BaseItem;
using System.Data.SqlClient;
using DataTable = System.Data.DataTable;
using System.Reflection;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using NPOI.SS.Formula.Functions;
using NPOI.HSSF.EventUserModel.DummyRecord;
using NPOI.XSSF.UserModel.Charts;

public partial class P010107030001 : PageBase
{

    #region event
    //20160907 (U) by Tank, RQ-2016-017288-000 新邏輯與舊邏輯判斷用Flag
    private string strFlag = string.Empty;

    protected void Page_Load(object sender, EventArgs e)
    {
        if(!IsPostBack)
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

        //20170412 (U) by Tank, 設定ScriptManager's AsyncPostBackTimeout為30分鐘(180秒)
        this.ScriptManager1.AsyncPostBackTimeout = 1800;
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        //20160907 (U) by Tank, RQ-2016-017288-000 新邏輯與舊邏輯判斷用Flag
        lblstrFlag.Text = "O";
        

        this.gpList.CurrentPageIndex = 0;
        BindGridView();
        this.gpList.Visible = true;
    }

    protected void btnSearch_New_Click(object sender, EventArgs e)
    {
        //20160907 (U) by Tank, RQ-2016-017288-000 新邏輯與舊邏輯判斷用Flag
        lblstrFlag.Text = "N";

        this.gpList.CurrentPageIndex = 0;
        BindGridView();
        this.gpList.Visible = true;
    }

    protected void gpList_PageChanged(object src, PageChangedEventArgs e)
    {
        this.gpList.CurrentPageIndex = e.NewPageIndex;
        this.BindGridView();
    }

    #region 產出檔案
    //產出EXCEL
    protected void btnExportEXCLE_Click(object sender, EventArgs e)
    {
        string strMsgID = string.Empty;
        string strServerPathFile = this.Server.MapPath(UtilHelper.GetAppSettings("ExportExcelFilePath").ToString());

        //20160601 (U) by Tank, 改用loacl的ExportRepeatReport
        //if (BRASExport.ExportRepeatReport(ref strServerPathFile, ref strMsgID))
        //20160907 (U) by Tank, RQ-2016-017288-000 add 參數strFlag
        strFlag = lblstrFlag.Text;
        if (ExportRepeatReport(this.dpBatchDate.Text.Replace("/", ""), ref strServerPathFile, ref strMsgID, strFlag))
        {
            //* 顯示提示訊息：匯出到Excel文檔資料成功
            string strFileName = "重覆件報表-人工.xls";
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
    #endregion 產出檔案
    #endregion event

    #region function
    /// <summary>
    /// 從Show.xml取漢字，設置畫面控件的Text
    /// </summary>
    private void ShowControlsText()
    {
        this.gvpbSearchRecord.Columns[0].HeaderText = BaseHelper.GetShowText("01_01070300_004");
        this.gvpbSearchRecord.Columns[1].HeaderText = BaseHelper.GetShowText("01_01070300_005");
        this.gvpbSearchRecord.Columns[2].HeaderText = BaseHelper.GetShowText("01_01070300_006");
        this.gvpbSearchRecord.Columns[3].HeaderText = BaseHelper.GetShowText("01_01070300_007");
        this.gvpbSearchRecord.Columns[4].HeaderText = BaseHelper.GetShowText("01_01070300_008");
        this.gvpbSearchRecord.Columns[5].HeaderText = BaseHelper.GetShowText("01_01070300_009");
        this.gvpbSearchRecord.Columns[6].HeaderText = BaseHelper.GetShowText("01_01070300_010");
        this.gvpbSearchRecord.Columns[7].HeaderText = BaseHelper.GetShowText("01_01070300_011");
        this.gvpbSearchRecord.Columns[8].HeaderText = BaseHelper.GetShowText("01_01070300_012");
        this.gvpbSearchRecord.Columns[9].HeaderText = BaseHelper.GetShowText("01_01070300_013");
        this.gvpbSearchRecord.Columns[10].HeaderText = BaseHelper.GetShowText("01_01070300_014");
        this.gvpbSearchRecord.Columns[11].HeaderText = BaseHelper.GetShowText("01_01070300_015");
        this.gvpbSearchRecord.Columns[12].HeaderText = BaseHelper.GetShowText("01_01070300_016");
        this.gvpbSearchRecord.Columns[13].HeaderText = BaseHelper.GetShowText("01_01070300_017");
        this.gvpbSearchRecord.Columns[14].HeaderText = BaseHelper.GetShowText("01_01070300_018");
        this.gvpbSearchRecord.Columns[15].HeaderText = BaseHelper.GetShowText("01_01070300_019");
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
    /// </summary>
    private void BindGridView()
    {
        try
        {
            string strMsgID = string.Empty;
            int intTotolCount = 0;
            //20160601 (U) by Tank, add 最後重覆日
            //DataTable dt = BRASExport.SearchRepeatReport(ref strMsgID, this.gpList.CurrentPageIndex, this.gpList.PageSize, ref intTotolCount);
            //20160907 (U) by Tank, RQ-2016-017288-000 add 參數strFlag
            strFlag = lblstrFlag.Text;
            DataTable dt = SearchRepeatReport(ref strMsgID, this.dpBatchDate.Text.Replace("/",""), this.gpList.CurrentPageIndex, this.gpList.PageSize, ref intTotolCount, this.strFlag);
            if (dt == null)
            {
                this.gpList.RecordCount = 0;
                this.gvpbSearchRecord.DataSource = null;
                this.gvpbSearchRecord.DataBind();
            }
            else
            {
                this.gpList.RecordCount = intTotolCount;
                this.gvpbSearchRecord.DataSource = dt;
                this.gvpbSearchRecord.DataBind();
            }
            //* 顯示端末訊息
            base.strClientMsg += MessageHelper.GetMessage(strMsgID);
        }
        catch (Exception exp)
        {
            Logging.Log(exp, LogLayer.UI);
            //* 顯示端末訊息
            base.strClientMsg += MessageHelper.GetMessage("01_01070300_001");
        }
    }

    //20160601 (U) by Tank, 從BRASExport取出 [重覆件報表]區塊 修改
    #region 重覆件報表
    //2021/03/09_Ares_Stanley-DB名稱改為變數
    private const string SEARCH_RepeatReport = @" --一般簽單
                                                Select * From (
                                                    SELECT f.MaxRepeatDay, a.Batch_Date, a.Batch_NO, a.Receive_Batch, a.Shop_ID, 
                                                    CASE WHEN a.Sign_Type='1' THEN '一般' ELSE '分期' END AS Sign_Type, 
                                                    CASE WHEN d.Process_Flag='01' THEN d.Process_Flag + ' 已匯檔' ELSE d.Process_Flag + ' 已產檔' END AS Process_Flag, 
                                                    a.SN, a.Card_No, a.Tran_Date, a.Installment_Periods, a.Auth_Code, a.AMT, 
                                                    CASE WHEN a.Receipt_Type='40' THEN a.Receipt_Type + ' 請款' ELSE a.Receipt_Type + ' 退款' END AS Receipt_Type, 
                                                    (e.Create_User) AS [1Key_user], (a.Create_User) AS [2Key_user]
                                                    FROM (
                                                        SELECT DISTINCT c.*
                                                        FROM (
	                                                        SELECT a.*
	                                                        FROM [{3}].[dbo].[Artificial_Signing_Detail] a
	                                                        JOIN (
		                                                        SELECT Batch_Date, Receive_Batch, Batch_NO, Shop_ID, Sign_Type, KeyIn_Flag
		                                                        FROM [{3}].[dbo].[Artificial_Signing_Primary]
		                                                        WHERE KeyIn_Flag = '2'
			                                                        AND Balance_Flag = 'Y'
			                                                        AND KeyIn_MatchFlag = 'Y'
			                                                        AND Batch_Date IN (
				                                                        SELECT DISTINCT TOP {0} Batch_Date
				                                                        FROM [{3}].[dbo].[Artificial_Signing_Detail]
				                                                        ORDER BY Batch_Date DESC
				                                                        )
			                                                        AND Sign_Type = '1'
		                                                        ) b ON a.Batch_Date = b.Batch_Date
		                                                        AND a.Receive_Batch = b.Receive_Batch
		                                                        AND a.Batch_NO = b.Batch_NO
		                                                        AND a.Shop_ID = b.Shop_ID
		                                                        AND a.KeyIn_Flag = b.KeyIn_Flag
		                                                        AND a.Sign_Type = b.Sign_Type
	                                                        ) c
                                                        JOIN (
	                                                        SELECT a.*
	                                                        FROM [{3}].[dbo].[Artificial_Signing_Detail] a
	                                                        JOIN (
		                                                        SELECT Batch_Date, Receive_Batch, Batch_NO, Shop_ID, Sign_Type, KeyIn_Flag
		                                                        FROM [{3}].[dbo].[Artificial_Signing_Primary]
		                                                        WHERE KeyIn_Flag = '2'
			                                                        AND Balance_Flag = 'Y'
			                                                        AND KeyIn_MatchFlag = 'Y'
			                                                        AND Batch_Date IN (
				                                                        SELECT DISTINCT TOP {0} Batch_Date
				                                                        FROM [{3}].[dbo].[Artificial_Signing_Detail]
				                                                        ORDER BY Batch_Date DESC
				                                                        )
			                                                        AND Sign_Type = '1'
		                                                        ) b ON a.Batch_Date = b.Batch_Date
		                                                        AND a.Receive_Batch = b.Receive_Batch
		                                                        AND a.Batch_NO = b.Batch_NO
		                                                        AND a.Shop_ID = b.Shop_ID
		                                                        AND a.KeyIn_Flag = b.KeyIn_Flag
		                                                        AND a.Sign_Type = b.Sign_Type
	                                                        ) d ON c.Shop_ID = d.Shop_ID
	                                                        AND c.Card_No = d.Card_No
	                                                        AND c.Tran_Date = d.Tran_Date
	                                                        AND c.Auth_Code = d.Auth_Code
	                                                        AND c.AMT = d.AMT
	                                                        AND c.Receipt_Type = d.Receipt_Type
	                                                        AND (
		                                                        c.Batch_Date <> d.Batch_Date
		                                                        OR c.Receive_Batch <> d.Receive_Batch
		                                                        OR c.Batch_NO <> d.Batch_NO
		                                                        )
                                                        ) a
                                                    LEFT JOIN [{3}].[dbo].[Artificial_Signing_Batch_Data] d ON a.Batch_Date = d.Batch_Date
                                                        AND a.Receive_Batch = d.Receive_Batch
                                                        AND a.Batch_NO = d.Batch_NO
                                                        AND a.Shop_ID = d.Shop_ID
                                                        AND a.Sign_Type = d.Sign_Type
                                                    LEFT JOIN [{3}].[dbo].[Artificial_Signing_Detail] e ON e.KeyIn_Flag = '1'
                                                        AND a.Batch_Date = e.Batch_Date
                                                        AND a.Shop_ID = e.Shop_ID
                                                        AND a.Batch_NO = e.Batch_NO
                                                        AND a.Receive_Batch = e.Receive_Batch
                                                        AND a.SN = e.SN
                                                        AND a.CASe_Status = e.CASe_Status
                                                        AND a.Sign_Type = e.Sign_Type
                                                    LEFT JOIN (
                                                        SELECT MAX(Batch_Date) MaxRepeatDay, Shop_ID, Card_No, Tran_Date, Auth_Code, AMT, Receipt_Type
                                                        FROM (
	                                                        SELECT a.*
	                                                        FROM [{3}].[dbo].[Artificial_Signing_Detail] a
	                                                        JOIN (
		                                                        SELECT Batch_Date, Receive_Batch, Batch_NO, Shop_ID, Sign_Type, KeyIn_Flag
		                                                        FROM [{3}].[dbo].[Artificial_Signing_Primary]
		                                                        WHERE KeyIn_Flag = '2'
			                                                        AND Balance_Flag = 'Y'
			                                                        AND KeyIn_MatchFlag = 'Y'
			                                                        AND Batch_Date IN (
				                                                        SELECT DISTINCT TOP {0} Batch_Date
				                                                        FROM [{3}].[dbo].[Artificial_Signing_Detail]
				                                                        ORDER BY Batch_Date DESC
				                                                        )
			                                                        AND Sign_Type = '1'
		                                                        ) b ON a.Batch_Date = b.Batch_Date
		                                                        AND a.Receive_Batch = b.Receive_Batch
		                                                        AND a.Batch_NO = b.Batch_NO
		                                                        AND a.Shop_ID = b.Shop_ID
		                                                        AND a.KeyIn_Flag = b.KeyIn_Flag
		                                                        AND a.Sign_Type = b.Sign_Type
	                                                        ) c
                                                        GROUP BY Shop_ID, Card_No, Tran_Date, Auth_Code, AMT, Receipt_Type
                                                        HAVING count(*) > 1
                                                        ) f ON a.Shop_ID = f.Shop_ID
                                                        AND a.Card_No = f.Card_No
                                                        AND a.Tran_Date = f.Tran_Date
                                                        AND a.Auth_Code = f.Auth_Code
                                                        AND a.AMT = f.AMT
                                                        AND a.Receipt_Type = f.Receipt_Type
                                                    	
                                                    UNION ALL

                                                    --分期簽單
                                                    SELECT f.MaxRepeatDay, a.Batch_Date, a.Batch_NO, a.Receive_Batch, a.Shop_ID, 
                                                    CASE WHEN a.Sign_Type='1' THEN '一般' ELSE '分期' END AS Sign_Type, 
                                                    CASE WHEN d.Process_Flag='01' THEN d.Process_Flag + ' 已匯檔' ELSE d.Process_Flag + ' 已產檔' END AS Process_Flag, 
                                                    a.SN, a.Card_No, a.Tran_Date, a.Installment_Periods, a.Auth_Code, a.AMT, 
                                                    CASE WHEN a.Receipt_Type='40' THEN a.Receipt_Type + ' 請款' ELSE a.Receipt_Type + ' 退款' END AS Receipt_Type, 
                                                    (e.Create_User) AS [1Key_user], (a.Create_User) AS [2Key_user]
                                                    FROM (
                                                        SELECT DISTINCT c.*
                                                        FROM (
	                                                        SELECT a.*
	                                                        FROM [{3}].[dbo].[Artificial_Signing_Detail] a
	                                                        JOIN (
		                                                        SELECT Batch_Date, Receive_Batch, Batch_NO, Shop_ID, Sign_Type, KeyIn_Flag
		                                                        FROM [{3}].[dbo].[Artificial_Signing_Primary]
		                                                        WHERE KeyIn_Flag = '2'
			                                                        AND Balance_Flag = 'Y'
			                                                        AND KeyIn_MatchFlag = 'Y'
			                                                        AND Batch_Date IN (
				                                                        SELECT DISTINCT TOP {1} Batch_Date
				                                                        FROM [{3}].[dbo].[Artificial_Signing_Detail]
				                                                        ORDER BY Batch_Date DESC
				                                                        )
			                                                        AND Sign_Type = '2'
		                                                        ) b ON a.Batch_Date = b.Batch_Date
		                                                        AND a.Receive_Batch = b.Receive_Batch
		                                                        AND a.Batch_NO = b.Batch_NO
		                                                        AND a.Shop_ID = b.Shop_ID
		                                                        AND a.KeyIn_Flag = b.KeyIn_Flag
		                                                        AND a.Sign_Type = b.Sign_Type
	                                                        ) c
                                                        JOIN (
	                                                        SELECT a.*
	                                                        FROM [{3}].[dbo].[Artificial_Signing_Detail] a
	                                                        JOIN (
		                                                        SELECT Batch_Date, Receive_Batch, Batch_NO, Shop_ID, Sign_Type, KeyIn_Flag
		                                                        FROM [{3}].[dbo].[Artificial_Signing_Primary]
		                                                        WHERE KeyIn_Flag = '2'
			                                                        AND Balance_Flag = 'Y'
			                                                        AND KeyIn_MatchFlag = 'Y'
			                                                        AND Batch_Date IN (
				                                                        SELECT DISTINCT TOP {1} Batch_Date
				                                                        FROM [{3}].[dbo].[Artificial_Signing_Detail]
				                                                        ORDER BY Batch_Date DESC
				                                                        )
			                                                        AND Sign_Type = '2'
		                                                        ) b ON a.Batch_Date = b.Batch_Date
		                                                        AND a.Receive_Batch = b.Receive_Batch
		                                                        AND a.Batch_NO = b.Batch_NO
		                                                        AND a.Shop_ID = b.Shop_ID
		                                                        AND a.KeyIn_Flag = b.KeyIn_Flag
		                                                        AND a.Sign_Type = b.Sign_Type
	                                                        ) d ON c.Shop_ID = d.Shop_ID
	                                                        AND c.Card_No = d.Card_No
	                                                        AND c.Tran_Date = d.Tran_Date
	                                                        AND c.Auth_Code = d.Auth_Code
	                                                        AND c.AMT = d.AMT
	                                                        AND c.Receipt_Type = d.Receipt_Type
	                                                        AND c.Product_Type = d.Product_Type
	                                                        AND c.Installment_Periods = d.Installment_Periods
	                                                        AND (
		                                                        c.Batch_Date <> d.Batch_Date
		                                                        OR c.Receive_Batch <> d.Receive_Batch
		                                                        OR c.Batch_NO <> d.Batch_NO
		                                                        )
                                                        ) a
                                                    LEFT JOIN [{3}].[dbo].[Artificial_Signing_Batch_Data] d ON a.Batch_Date = d.Batch_Date
                                                        AND a.Receive_Batch = d.Receive_Batch
                                                        AND a.Batch_NO = d.Batch_NO
                                                        AND a.Shop_ID = d.Shop_ID
                                                        AND a.Sign_Type = d.Sign_Type
                                                    LEFT JOIN [{3}].[dbo].[Artificial_Signing_Detail] e ON e.KeyIn_Flag = '1'
                                                        AND a.Batch_Date = e.Batch_Date
                                                        AND a.Shop_ID = e.Shop_ID
                                                        AND a.Batch_NO = e.Batch_NO
                                                        AND a.Receive_Batch = e.Receive_Batch
                                                        AND a.SN = e.SN
                                                        AND a.CASe_Status = e.CASe_Status
                                                        AND a.Sign_Type = e.Sign_Type
                                                    LEFT JOIN (
                                                        SELECT MAX(Batch_Date) MaxRepeatDay, Shop_ID, Card_No, Tran_Date, Auth_Code, AMT, Receipt_Type, Product_Type, Installment_Periods
                                                        FROM (
	                                                        SELECT a.*
	                                                        FROM [{3}].[dbo].[Artificial_Signing_Detail] a
	                                                        JOIN (
		                                                        SELECT Batch_Date, Receive_Batch, Batch_NO, Shop_ID, Sign_Type, KeyIn_Flag
		                                                        FROM [{3}].[dbo].[Artificial_Signing_Primary]
		                                                        WHERE KeyIn_Flag = '2'
			                                                        AND Balance_Flag = 'Y'
			                                                        AND KeyIn_MatchFlag = 'Y'
			                                                        AND Batch_Date IN (
				                                                        SELECT DISTINCT TOP {1} Batch_Date
				                                                        FROM [{3}].[dbo].[Artificial_Signing_Detail]
				                                                        ORDER BY Batch_Date DESC
				                                                        )
			                                                        AND Sign_Type = '2'
		                                                        ) b ON a.Batch_Date = b.Batch_Date
		                                                        AND a.Receive_Batch = b.Receive_Batch
		                                                        AND a.Batch_NO = b.Batch_NO
		                                                        AND a.Shop_ID = b.Shop_ID
		                                                        AND a.KeyIn_Flag = b.KeyIn_Flag
		                                                        AND a.Sign_Type = b.Sign_Type
	                                                        ) c
                                                        GROUP BY Shop_ID, Card_No, Tran_Date, Auth_Code, AMT, Receipt_Type, Product_Type, Installment_Periods
                                                        HAVING count(*) > 1
                                                        ) f ON a.Shop_ID = f.Shop_ID
                                                        AND a.Card_No = f.Card_No
                                                        AND a.Tran_Date = f.Tran_Date
                                                        AND a.Auth_Code = f.Auth_Code
                                                        AND a.AMT = f.AMT
                                                        AND a.Receipt_Type = f.Receipt_Type
                                                    ) dt Where dt.MaxRepeatDay = {2}
                                                    ORDER BY Sign_Type,Shop_ID,Card_No,Tran_Date,Auth_Code,Installment_Periods,AMT,Receipt_Type,MaxRepeatDay, Batch_Date DESC ";

    //20160907 (U) by Tank, RQ-2016-017288-000 新邏輯
    private const string SEARCH_RepeatReport_New = @" --一般簽單
                                                Select * From (
                                                    SELECT f.MaxRepeatDay, a.Batch_Date, a.Batch_NO, a.Receive_Batch, a.Shop_ID, 
                                                    CASE WHEN a.Sign_Type='1' THEN '一般' ELSE '分期' END AS Sign_Type, 
                                                    CASE WHEN d.Process_Flag='01' THEN d.Process_Flag + ' 已匯檔' ELSE d.Process_Flag + ' 已產檔' END AS Process_Flag, 
                                                    a.SN, a.Card_No, a.Tran_Date, a.Installment_Periods, a.Auth_Code, a.AMT, 
                                                    CASE WHEN a.Receipt_Type='40' THEN a.Receipt_Type + ' 請款' ELSE a.Receipt_Type + ' 退款' END AS Receipt_Type, 
                                                    (e.Create_User) AS [1Key_user], (a.Create_User) AS [2Key_user]
                                                    FROM (
                                                        SELECT DISTINCT c.*
                                                        FROM (
	                                                        SELECT a.*
	                                                        FROM [{3}].[dbo].[Artificial_Signing_Detail] a
	                                                        JOIN (
		                                                        SELECT Batch_Date, Receive_Batch, Batch_NO, Shop_ID, Sign_Type, KeyIn_Flag
		                                                        FROM [{3}].[dbo].[Artificial_Signing_Primary]
		                                                        WHERE KeyIn_Flag = '2'
			                                                        AND Balance_Flag = 'Y'
			                                                        AND KeyIn_MatchFlag = 'Y'
			                                                        AND Batch_Date IN (
				                                                        SELECT DISTINCT TOP {0} Batch_Date
				                                                        FROM [{3}].[dbo].[Artificial_Signing_Detail]
				                                                        ORDER BY Batch_Date DESC
				                                                        )
			                                                        AND Sign_Type = '1'
		                                                        ) b ON a.Batch_Date = b.Batch_Date
		                                                        AND a.Receive_Batch = b.Receive_Batch
		                                                        AND a.Batch_NO = b.Batch_NO
		                                                        AND a.Shop_ID = b.Shop_ID
		                                                        AND a.KeyIn_Flag = b.KeyIn_Flag
		                                                        AND a.Sign_Type = b.Sign_Type
	                                                        ) c
                                                        JOIN (
	                                                        SELECT a.*
	                                                        FROM [{3}].[dbo].[Artificial_Signing_Detail] a
	                                                        JOIN (
		                                                        SELECT Batch_Date, Receive_Batch, Batch_NO, Shop_ID, Sign_Type, KeyIn_Flag
		                                                        FROM [{3}].[dbo].[Artificial_Signing_Primary]
		                                                        WHERE KeyIn_Flag = '2'
			                                                        AND Balance_Flag = 'Y'
			                                                        AND KeyIn_MatchFlag = 'Y'
			                                                        AND Batch_Date IN (
				                                                        SELECT DISTINCT TOP {0} Batch_Date
				                                                        FROM [{3}].[dbo].[Artificial_Signing_Detail]
				                                                        ORDER BY Batch_Date DESC
				                                                        )
			                                                        AND Sign_Type = '1'
		                                                        ) b ON a.Batch_Date = b.Batch_Date
		                                                        AND a.Receive_Batch = b.Receive_Batch
		                                                        AND a.Batch_NO = b.Batch_NO
		                                                        AND a.Shop_ID = b.Shop_ID
		                                                        AND a.KeyIn_Flag = b.KeyIn_Flag
		                                                        AND a.Sign_Type = b.Sign_Type
	                                                        ) d ON c.Shop_ID = d.Shop_ID
	                                                        AND c.Card_No = d.Card_No
	                                                        AND c.Tran_Date = d.Tran_Date
	                                                        AND c.Auth_Code = d.Auth_Code
	                                                        AND c.AMT = d.AMT
	                                                        AND c.Receipt_Type = d.Receipt_Type
	                                                        --20160907 (U) by Tank, 調整重覆件邏輯，不需管編批日、批號、收件批次
                                                            /*AND (
		                                                        c.Batch_Date <> d.Batch_Date
		                                                        OR c.Receive_Batch <> d.Receive_Batch
		                                                        OR c.Batch_NO <> d.Batch_NO
		                                                        )*/
                                                        ) a
                                                    LEFT JOIN [{3}].[dbo].[Artificial_Signing_Batch_Data] d ON a.Batch_Date = d.Batch_Date
                                                        AND a.Receive_Batch = d.Receive_Batch
                                                        AND a.Batch_NO = d.Batch_NO
                                                        AND a.Shop_ID = d.Shop_ID
                                                        AND a.Sign_Type = d.Sign_Type
                                                    LEFT JOIN [{3}].[dbo].[Artificial_Signing_Detail] e ON e.KeyIn_Flag = '1'
                                                        AND a.Batch_Date = e.Batch_Date
                                                        AND a.Shop_ID = e.Shop_ID
                                                        AND a.Batch_NO = e.Batch_NO
                                                        AND a.Receive_Batch = e.Receive_Batch
                                                        AND a.SN = e.SN
                                                        AND a.CASe_Status = e.CASe_Status
                                                        AND a.Sign_Type = e.Sign_Type
                                                    LEFT JOIN (
                                                        SELECT MAX(Batch_Date) MaxRepeatDay, Shop_ID, Card_No, Tran_Date, Auth_Code, AMT, Receipt_Type
                                                        FROM (
	                                                        SELECT a.*
	                                                        FROM [{3}].[dbo].[Artificial_Signing_Detail] a
	                                                        JOIN (
		                                                        SELECT Batch_Date, Receive_Batch, Batch_NO, Shop_ID, Sign_Type, KeyIn_Flag
		                                                        FROM [{3}].[dbo].[Artificial_Signing_Primary]
		                                                        WHERE KeyIn_Flag = '2'
			                                                        AND Balance_Flag = 'Y'
			                                                        AND KeyIn_MatchFlag = 'Y'
			                                                        AND Batch_Date IN (
				                                                        SELECT DISTINCT TOP {0} Batch_Date
				                                                        FROM [{3}].[dbo].[Artificial_Signing_Detail]
				                                                        ORDER BY Batch_Date DESC
				                                                        )
			                                                        AND Sign_Type = '1'
		                                                        ) b ON a.Batch_Date = b.Batch_Date
		                                                        AND a.Receive_Batch = b.Receive_Batch
		                                                        AND a.Batch_NO = b.Batch_NO
		                                                        AND a.Shop_ID = b.Shop_ID
		                                                        AND a.KeyIn_Flag = b.KeyIn_Flag
		                                                        AND a.Sign_Type = b.Sign_Type
	                                                        ) c
                                                        GROUP BY Shop_ID, Card_No, Tran_Date, Auth_Code, AMT, Receipt_Type
                                                        HAVING count(*) > 1
                                                        ) f ON a.Shop_ID = f.Shop_ID
                                                        AND a.Card_No = f.Card_No
                                                        AND a.Tran_Date = f.Tran_Date
                                                        AND a.Auth_Code = f.Auth_Code
                                                        AND a.AMT = f.AMT
                                                        AND a.Receipt_Type = f.Receipt_Type
                                                    	
                                                    UNION ALL

                                                    --分期簽單
                                                    SELECT f.MaxRepeatDay, a.Batch_Date, a.Batch_NO, a.Receive_Batch, a.Shop_ID, 
                                                    CASE WHEN a.Sign_Type='1' THEN '一般' ELSE '分期' END AS Sign_Type, 
                                                    CASE WHEN d.Process_Flag='01' THEN d.Process_Flag + ' 已匯檔' ELSE d.Process_Flag + ' 已產檔' END AS Process_Flag, 
                                                    a.SN, a.Card_No, a.Tran_Date, a.Installment_Periods, a.Auth_Code, a.AMT, 
                                                    CASE WHEN a.Receipt_Type='40' THEN a.Receipt_Type + ' 請款' ELSE a.Receipt_Type + ' 退款' END AS Receipt_Type, 
                                                    (e.Create_User) AS [1Key_user], (a.Create_User) AS [2Key_user]
                                                    FROM (
                                                        SELECT DISTINCT c.*
                                                        FROM (
	                                                        SELECT a.*
	                                                        FROM [{3}].[dbo].[Artificial_Signing_Detail] a
	                                                        JOIN (
		                                                        SELECT Batch_Date, Receive_Batch, Batch_NO, Shop_ID, Sign_Type, KeyIn_Flag
		                                                        FROM [{3}].[dbo].[Artificial_Signing_Primary]
		                                                        WHERE KeyIn_Flag = '2'
			                                                        AND Balance_Flag = 'Y'
			                                                        AND KeyIn_MatchFlag = 'Y'
			                                                        AND Batch_Date IN (
				                                                        SELECT DISTINCT TOP {1} Batch_Date
				                                                        FROM [{3}].[dbo].[Artificial_Signing_Detail]
				                                                        ORDER BY Batch_Date DESC
				                                                        )
			                                                        AND Sign_Type = '2'
		                                                        ) b ON a.Batch_Date = b.Batch_Date
		                                                        AND a.Receive_Batch = b.Receive_Batch
		                                                        AND a.Batch_NO = b.Batch_NO
		                                                        AND a.Shop_ID = b.Shop_ID
		                                                        AND a.KeyIn_Flag = b.KeyIn_Flag
		                                                        AND a.Sign_Type = b.Sign_Type
	                                                        ) c
                                                        JOIN (
	                                                        SELECT a.*
	                                                        FROM [{3}].[dbo].[Artificial_Signing_Detail] a
	                                                        JOIN (
		                                                        SELECT Batch_Date, Receive_Batch, Batch_NO, Shop_ID, Sign_Type, KeyIn_Flag
		                                                        FROM [{3}].[dbo].[Artificial_Signing_Primary]
		                                                        WHERE KeyIn_Flag = '2'
			                                                        AND Balance_Flag = 'Y'
			                                                        AND KeyIn_MatchFlag = 'Y'
			                                                        AND Batch_Date IN (
				                                                        SELECT DISTINCT TOP {1} Batch_Date
				                                                        FROM [{3}].[dbo].[Artificial_Signing_Detail]
				                                                        ORDER BY Batch_Date DESC
				                                                        )
			                                                        AND Sign_Type = '2'
		                                                        ) b ON a.Batch_Date = b.Batch_Date
		                                                        AND a.Receive_Batch = b.Receive_Batch
		                                                        AND a.Batch_NO = b.Batch_NO
		                                                        AND a.Shop_ID = b.Shop_ID
		                                                        AND a.KeyIn_Flag = b.KeyIn_Flag
		                                                        AND a.Sign_Type = b.Sign_Type
	                                                        ) d ON c.Shop_ID = d.Shop_ID
	                                                        AND c.Card_No = d.Card_No
	                                                        AND c.Tran_Date = d.Tran_Date
	                                                        AND c.Auth_Code = d.Auth_Code
	                                                        AND c.AMT = d.AMT
	                                                        AND c.Receipt_Type = d.Receipt_Type
	                                                        AND c.Product_Type = d.Product_Type
	                                                        AND c.Installment_Periods = d.Installment_Periods
	                                                        --20160907 (U) by Tank, 調整重覆件邏輯，不需管編批日、批號、收件批次
                                                            /*AND (
		                                                        c.Batch_Date <> d.Batch_Date
		                                                        OR c.Receive_Batch <> d.Receive_Batch
		                                                        OR c.Batch_NO <> d.Batch_NO
		                                                        )*/
                                                        ) a
                                                    LEFT JOIN [{3}].[dbo].[Artificial_Signing_Batch_Data] d ON a.Batch_Date = d.Batch_Date
                                                        AND a.Receive_Batch = d.Receive_Batch
                                                        AND a.Batch_NO = d.Batch_NO
                                                        AND a.Shop_ID = d.Shop_ID
                                                        AND a.Sign_Type = d.Sign_Type
                                                    LEFT JOIN [{3}].[dbo].[Artificial_Signing_Detail] e ON e.KeyIn_Flag = '1'
                                                        AND a.Batch_Date = e.Batch_Date
                                                        AND a.Shop_ID = e.Shop_ID
                                                        AND a.Batch_NO = e.Batch_NO
                                                        AND a.Receive_Batch = e.Receive_Batch
                                                        AND a.SN = e.SN
                                                        AND a.CASe_Status = e.CASe_Status
                                                        AND a.Sign_Type = e.Sign_Type
                                                    LEFT JOIN (
                                                        SELECT MAX(Batch_Date) MaxRepeatDay, Shop_ID, Card_No, Tran_Date, Auth_Code, AMT, Receipt_Type, Product_Type, Installment_Periods
                                                        FROM (
	                                                        SELECT a.*
	                                                        FROM [{3}].[dbo].[Artificial_Signing_Detail] a
	                                                        JOIN (
		                                                        SELECT Batch_Date, Receive_Batch, Batch_NO, Shop_ID, Sign_Type, KeyIn_Flag
		                                                        FROM [{3}].[dbo].[Artificial_Signing_Primary]
		                                                        WHERE KeyIn_Flag = '2'
			                                                        AND Balance_Flag = 'Y'
			                                                        AND KeyIn_MatchFlag = 'Y'
			                                                        AND Batch_Date IN (
				                                                        SELECT DISTINCT TOP {1} Batch_Date
				                                                        FROM [{3}].[dbo].[Artificial_Signing_Detail]
				                                                        ORDER BY Batch_Date DESC
				                                                        )
			                                                        AND Sign_Type = '2'
		                                                        ) b ON a.Batch_Date = b.Batch_Date
		                                                        AND a.Receive_Batch = b.Receive_Batch
		                                                        AND a.Batch_NO = b.Batch_NO
		                                                        AND a.Shop_ID = b.Shop_ID
		                                                        AND a.KeyIn_Flag = b.KeyIn_Flag
		                                                        AND a.Sign_Type = b.Sign_Type
	                                                        ) c
                                                        GROUP BY Shop_ID, Card_No, Tran_Date, Auth_Code, AMT, Receipt_Type, Product_Type, Installment_Periods
                                                        HAVING count(*) > 1
                                                        ) f ON a.Shop_ID = f.Shop_ID
                                                        AND a.Card_No = f.Card_No
                                                        AND a.Tran_Date = f.Tran_Date
                                                        AND a.Auth_Code = f.Auth_Code
                                                        AND a.AMT = f.AMT
                                                        AND a.Receipt_Type = f.Receipt_Type
                                                    ) dt Where dt.MaxRepeatDay = {2}
                                                    ORDER BY Sign_Type,Shop_ID,Card_No,Tran_Date,Auth_Code,Installment_Periods,AMT,Receipt_Type,MaxRepeatDay, Batch_Date DESC ";

    /// <summary>
    /// 重覆件報表 查詢
    /// </summary>
    /// <param name="strMsgID">返回的錯誤ID號</param>
    /// <returns>成功時：返回查詢結果；失敗時：null</returns>
    /// //20160601 (U) by Tank, 查詢條件add最後重覆日:strQueryDate
    /// //20160907 (U) by Tank, RQ-2016-017288-000 add 參數strFlag
    public static DataTable SearchRepeatReport(ref string strMsgID, string strQueryDate, int intPageIndex, int intPageSize, ref int intTotolCount, string strFlag)
    {
        try
        {
            #region 依據Request查詢資料庫

            //* 由屬性表取出 [一定匯檔日數]
            DataTable dtblProperty = null;
            if (!CSIPCommonModel.BusinessRules.BRM_PROPERTY_KEY.GetEnableProperty("01", "AS_COMPAREDAY", ref dtblProperty))
            {
                strMsgID = "01_01070300_006";
                return null;
            }

            //* 聲明SQL Command變量
            SqlCommand sqlcmSearchData = new SqlCommand();
            sqlcmSearchData.CommandType = CommandType.Text;

            //20161021 (U) by Tank, timeout=240 設時間為4分鐘
            sqlcmSearchData.CommandTimeout = 240;

            //20160907 (U) by Tank, RQ-2016-017288-000 新邏輯與舊邏輯判斷用Flag
            if (strFlag == "O")
            {
                //2021/03/09_Ares_Stanley-DB名稱改為變數
                sqlcmSearchData.CommandText = string.Format(SEARCH_RepeatReport, dtblProperty.Select("PROPERTY_CODE = 'General'")[0]["PROPERTY_NAME"]
                                                                        , dtblProperty.Select("PROPERTY_CODE = 'Installment'")[0]["PROPERTY_NAME"], strQueryDate, UtilHelper.GetAppSettings("DB_KeyinGUI"));
            }
            else 
            {
                sqlcmSearchData.CommandText = string.Format(SEARCH_RepeatReport_New, dtblProperty.Select("PROPERTY_CODE = 'General'")[0]["PROPERTY_NAME"]
                                                                        , dtblProperty.Select("PROPERTY_CODE = 'Installment'")[0]["PROPERTY_NAME"], strQueryDate, UtilHelper.GetAppSettings("DB_KeyinGUI"));
            }
            

            //* 查詢數據
            DataSet dstSearchData = BRASExport.SearchOnDataSet(sqlcmSearchData, intPageIndex, intPageSize, ref intTotolCount);
            if (null == dstSearchData)  //* 查詢數據失敗
            {
                strMsgID = "01_01070300_001";
                return null;
            }
            else
            {
                //* 查詢的數據不存在時
                if (dstSearchData.Tables[0].Rows.Count == 0)
                {
                    strMsgID = "01_01070300_002";
                    return null;
                }
            }
            //* 查詢成功
            strMsgID = "01_01070300_003";
            return dstSearchData.Tables[0];

            #endregion 依據Request查詢資料庫
        }
        catch (Exception exp)
        {
            BRCompareRpt.SaveLog(exp);
            strMsgID = "01_01070300_001";
            return null;
        }
    }

    /// <summary>
    /// 產生重覆件報表Excel
    /// 修改紀錄:2021/02/02_Ares_Stanley-調整日期格式; 2021/03/08_Ares_Stanley-新增查無資料時，列印空報表; 2021/08/17_Ares_Stanley-卡號隱碼
    /// </summary>
    /// <param name="strPathFile">服務器端生成的Excel文檔路徑</param>
    /// <param name="strMsgID">返回的錯誤ID號</param>
    /// <returns>Excel生成成功標示：True--成功；False--失敗</returns>
    public static bool ExportRepeatReport(string strQueryDate, ref string strPathFile, ref string strMsgID, string strFlag)
    {
        //解決"格式太舊或是類型程式庫無效。 (Exception from HRESULT: 0x80028018 (TYPE_E_INVDATAREAD))"問題用
        System.Globalization.CultureInfo oldCI = System.Threading.Thread.CurrentThread.CurrentCulture;
        System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");

        strMsgID = "";
        try
        {
            //* 檢查目錄，并刪除以前的文檔資料
            BRExcel_File.CheckDirectory(ref strPathFile);

            //* 取要下載的資料
            DataTable dt = getData_ExportRepeatReport(ref strMsgID, strQueryDate, strFlag);
            if (dt == null)
                return false;

            #region 新CODE
            string strRPTPathFile = AppDomain.CurrentDomain.BaseDirectory + UtilHelper.GetAppSettings("ReportTemplate") + "RepeatReport_Artificial.xls";
            FileStream fs = new FileStream(strRPTPathFile, FileMode.Open);
            HSSFWorkbook wb = new HSSFWorkbook(fs);
            #region 空報表格式
            HSSFCellStyle emptyContentFormat = (HSSFCellStyle)wb.CreateCellStyle(); //建立文字格式
            emptyContentFormat.VerticalAlignment = VerticalAlignment.Center; //垂直置中
            emptyContentFormat.Alignment = HorizontalAlignment.Center; //水平置中
            emptyContentFormat.DataFormat = HSSFDataFormat.GetBuiltinFormat("@"); //將儲存格內容設定為文字
            emptyContentFormat.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin; // 儲存格框線
            emptyContentFormat.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
            emptyContentFormat.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
            emptyContentFormat.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
            HSSFFont emptyContentFont = (HSSFFont)wb.CreateFont(); //建立文字樣式
            emptyContentFont.FontHeightInPoints = 12; //字體大小
            emptyContentFont.FontName = "新細明體"; //字型
            emptyContentFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold; //粗體
            emptyContentFormat.SetFont(emptyContentFont); //設定儲存格的文字樣式
            #endregion
            ISheet sheet = wb.GetSheet("工作表1");
            // 查詢資料不為空時，寫入查詢資料
            if (dt.Rows.Count != 0)
            {
                ExportExcelForNPOI(dt, ref wb, 2, "工作表1");
                for (int row = 2; row < sheet.LastRowNum + 1; row++)
                {
                    if (sheet.GetRow(row).GetCell(12).StringCellValue != "")
                    {
                        sheet.GetRow(row).GetCell(12).SetCellValue(int.Parse(sheet.GetRow(row).GetCell(12).StringCellValue).ToString("N0"));
                    }
                    if (sheet.GetRow(row).GetCell(0).StringCellValue != "")
                    {
                    sheet.GetRow(row).GetCell(0).SetCellValue(DateTime.ParseExact(sheet.GetRow(row).GetCell(0).StringCellValue, "yyyyMMdd", null).ToString("yyyy/MM/dd"));
                    }
                    if (sheet.GetRow(row).GetCell(1).StringCellValue != "")
                    {
                    sheet.GetRow(row).GetCell(1).SetCellValue(DateTime.ParseExact(sheet.GetRow(row).GetCell(1).StringCellValue, "yyyyMMdd", null).ToString("yyyy/MM/dd"));
                    }
                    if (sheet.GetRow(row).GetCell(9).StringCellValue != "")
                    {
                    sheet.GetRow(row).GetCell(9).SetCellValue(DateTime.ParseExact(sheet.GetRow(row).GetCell(9).StringCellValue, "yyyyMMdd", null).ToString("yyyy/MM/dd"));
                    }
                }
                // 卡號隱碼
                for(int row = 2; row < sheet.LastRowNum+1; row++)
                {
                    sheet.GetRow(row).GetCell(8).SetCellValue(BRExcel_File.addHideCode(sheet.GetRow(row).GetCell(8).StringCellValue));
                }
            }
            else //查詢資料為空時，列印空報表
            {
                for (int col = 0; col < 16; col++)
                {
                    sheet.GetRow(2).CreateCell(col).CellStyle = emptyContentFormat;
                }
                sheet.AddMergedRegion(new CellRangeAddress(2, 2, 0, 15));
                sheet.GetRow(2).GetCell(0).SetCellValue("查無資料,查詢結束!");
            }
            #endregion 新CODE
            //* 保存文件到程序运行目录下
            strPathFile = strPathFile + @"\RepeatReport_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
            FileStream fs1 = new FileStream(strPathFile, FileMode.Create);
            wb.Write(fs1);
            fs1.Close();
            fs.Close();
            // 根據是否為空報表顯示提醒訊息
            if (dt.Rows.Count != 0)
            {
                strMsgID = "01_01070300_005";
            }
            else
            {
                strMsgID = "01_01070300_007";
            }
            return true;
        }
        catch (Exception ex)
        {
            strMsgID = "01_01070300_004";
            Logging.Log(ex, LogLayer.BusinessRule);
            throw ex;
        }
        finally
        {
            //解決"格式太舊或是類型程式庫無效。 (Exception from HRESULT: 0x80028018 (TYPE_E_INVDATAREAD))"問題用
            System.Threading.Thread.CurrentThread.CurrentCulture = oldCI;
        }
    }
    /// <summary>
    /// 產生重覆件報表Excel時，查詢數據
    /// </summary>
    /// <param name="strMsgID">返回的錯誤ID號</param>
    /// <returns>成功時：返回查詢結果；失敗時：null</returns>
    public static DataTable getData_ExportRepeatReport(ref string strMsgID, string strQueryDate, string strFlag)
    {
        try
        {
            #region 依據Request查詢資料庫

            //* 由屬性表取出 [一定匯檔日數]
            DataTable dtblProperty = null;
            if (!CSIPCommonModel.BusinessRules.BRM_PROPERTY_KEY.GetEnableProperty("01", "AS_COMPAREDAY", ref dtblProperty))
            {
                strMsgID = "01_01070300_006";
                return null;
            }

            //* 聲明SQL Command變量
            SqlCommand sqlcmSearchData = new SqlCommand();
            sqlcmSearchData.CommandType = CommandType.Text;

            //20161021 (U) by Tank, timeout=240 設時間為4分鐘
            sqlcmSearchData.CommandTimeout = 240;

            //sqlcmSearchData.CommandText = string.Format(SEARCH_RepeatReport, dtblProperty.Select("PROPERTY_CODE = 'General'")[0]["PROPERTY_NAME"]
            //                                                            , dtblProperty.Select("PROPERTY_CODE = 'Installment'")[0]["PROPERTY_NAME"], strQueryDate);

            //20160907 (U) by Tank, RQ-2016-017288-000 新邏輯與舊邏輯判斷用Flag
            if (strFlag == "O")
            {
                sqlcmSearchData.CommandText = string.Format(SEARCH_RepeatReport, dtblProperty.Select("PROPERTY_CODE = 'General'")[0]["PROPERTY_NAME"]
                                                                        , dtblProperty.Select("PROPERTY_CODE = 'Installment'")[0]["PROPERTY_NAME"], strQueryDate, UtilHelper.GetAppSettings("DB_KeyinGUI"));
            }
            else
            {
                sqlcmSearchData.CommandText = string.Format(SEARCH_RepeatReport_New, dtblProperty.Select("PROPERTY_CODE = 'General'")[0]["PROPERTY_NAME"]
                                                                        , dtblProperty.Select("PROPERTY_CODE = 'Installment'")[0]["PROPERTY_NAME"], strQueryDate, UtilHelper.GetAppSettings("DB_KeyinGUI"));
            }

            //* 查詢數據
            DataSet dstSearchData = BRASExport.SearchOnDataSet(sqlcmSearchData);
            if (null == dstSearchData)  //* 查詢數據失敗
            {
                strMsgID = "01_01070300_001";
                return null;
            }
            else
            {
                //* 2021/03/08_Ares_Stanley-查無資料時，產出空報表
                //* 查詢的數據不存在時
                if (dstSearchData.Tables[0].Rows.Count == 0)
                {
                    strMsgID = "01_01070300_002";
                    return dstSearchData.Tables[0];
                }
            }
            //* 查詢成功
            strMsgID = "01_01070300_003";
            return dstSearchData.Tables[0];

            #endregion 依據Request查詢資料庫
        }
        catch (Exception exp)
        {
            BRCompareRpt.SaveLog(exp);
            strMsgID = "01_01070300_001";
            return null;
        }
    }
    #endregion

    #region 共用NPOI
    /// <summary>
    /// 修改紀錄:2021/01/12_Ares_Stanley-新增共用NPOI
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

    /// <summary>
    /// 修改紀錄: 2021/01/12_Ares_Stanley-新增共用NPOI
    /// </summary>
    /// <param name="dt"></param>
    /// <param name="wb"></param>
    /// <param name="start"></param>
    /// <param name="delColumn"></param>
    /// <param name="sheetName"></param>
    private static void ExportExcelForNPOI_filter(DataTable dt, ref HSSFWorkbook wb, Int32 start, Int32 delColumn, String sheetName)
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
            cs.DataFormat = HSSFDataFormat.GetBuiltinFormat("@"); //將儲存格內容設定為文字
            HSSFFont font1 = (HSSFFont)wb.CreateFont();
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
}
